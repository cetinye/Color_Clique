﻿using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Color_Clique
{
   public class PickerWheel : MonoBehaviour
   {
      [Header("References :")]
      [SerializeField] private GameObject linePrefab;
      [SerializeField] private Transform linesParent;

      [Space]
      [SerializeField] private Transform PickerWheelTransform;
      [SerializeField] private Transform wheelCircle;
      [SerializeField] private Transform transformToRotate;
      [SerializeField] private GameObject wheelPiecePrefab;
      [SerializeField] private Transform wheelPiecesParent;

      [Space]
      [Header("Sounds :")]
      [SerializeField] private AudioSource audioSource;
      [SerializeField] private AudioClip tickAudioClip;
      [SerializeField][Range(0f, 1f)] private float volume = .5f;
      [SerializeField][Range(-3f, 3f)] private float pitch = 1f;

      [Space]
      [Header("Picker wheel settings :")]
      public float spinDuration = 8;
      [SerializeField][Range(.2f, 2f)] private float wheelSize = 1f;

      [Space]
      [Header("Picker wheel pieces :")]
      public WheelPiece[] wheelPieces;
      [SerializeField] Image needle;
      public List<RectTransform> wheelPiecesList = new List<RectTransform>();

      // Events
      private UnityAction onSpinStartEvent;
      private UnityAction<WheelPiece> onSpinEndEvent;


      private bool _isSpinning = false;

      public bool IsSpinning { get { return _isSpinning; } }


      private Vector2 pieceMinSize = new Vector2(81f, 146f);
      private Vector2 pieceMaxSize = new Vector2(144f, 213f);
      private int piecesMin = 2;
      private int piecesMax = 12;

      private float pieceAngle;
      private float halfPieceAngle;
      private float halfPieceAngleWithPaddings;


      private double accumulatedWeight;
      private System.Random rand = new System.Random();

      private List<int> nonZeroChancesIndices = new List<int>();

      private Tween tween;
      float prevAngle, currentAngle;
      private int numberOfSlots;
      [SerializeField] private List<Sprite> items = new List<Sprite>();
      private List<Sprite> usedItems = new List<Sprite>();

      public void Initialize()
      {
         pieceAngle = 360 / numberOfSlots;
         halfPieceAngle = pieceAngle / 2f;
         halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);

         Generate();

         CalculateWeightsAndIndices();
         if (nonZeroChancesIndices.Count == 0)
            Debug.LogError("You can't set all pieces chance to zero");


         SetupAudio();

      }

      private void SetupAudio()
      {
         audioSource.clip = tickAudioClip;
         audioSource.volume = volume;
         audioSource.pitch = pitch;
      }

      private void Generate()
      {
         wheelPiecePrefab = InstantiatePiece();

         RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
         float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, numberOfSlots));
         float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, numberOfSlots));
         rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
         rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

         // for (int i = 0; i < numberOfSlots; i++)
         //    DrawPiece(i);

         // Destroy(wheelPiecePrefab);

         SpawnSlots();
      }

      private void SpawnSlots()
      {
         wheelPieces = new WheelPiece[numberOfSlots];

         for (int i = 0; i < numberOfSlots; i++)
         {
            wheelPieces[i] = new WheelPiece();
         }

         for (int i = 0; i < numberOfSlots; i++)
            DrawPiece(i);
      }

      private void DrawPiece(int index)
      {
         WheelPiece piece = wheelPieces[index];
         Transform pieceTrns = InstantiatePiece().transform.GetChild(0);

         piece.Icon = pieceTrns.GetChild(0).GetComponent<Image>().sprite = GetRandomItem();
         // pieceTrns.GetChild(1).GetComponent<Text>().text = piece.Label;
         // pieceTrns.GetChild(2).GetComponent<Text>().text = piece.Amount.ToString();
         wheelPiecesList.Add(pieceTrns.GetChild(0).GetComponent<RectTransform>());

         //Line
         Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent).transform;
         lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

         pieceTrns.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
      }

      private GameObject InstantiatePiece()
      {
         return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
      }

      private Sprite GetRandomItem()
      {
         Sprite item;

         do
         {
            item = items[Random.Range(0, items.Count)];

         } while (usedItems.Contains(item));

         usedItems.Add(item);
         return item;
      }

      public void AssignWheelVariables(int numberOfSlots, int rotationSpeed)
      {
         this.numberOfSlots = numberOfSlots;
         this.spinDuration = rotationSpeed;
      }

      public void Spin()
      {
         if (!_isSpinning)
         {
            _isSpinning = true;
            if (onSpinStartEvent != null)
               onSpinStartEvent.Invoke();

            // int index = GetRandomPieceIndex();
            int index = 0;
            WheelPiece piece = wheelPieces[index];

            if (piece.Chance == 0 && nonZeroChancesIndices.Count != 0)
            {
               index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
               piece = wheelPieces[index];
            }

            float angle = -(pieceAngle * index);

            float rightOffset = (angle - 0) % 360;
            float leftOffset = (angle + 0) % 360;

            float randomAngle = Random.Range(leftOffset, rightOffset);

            Vector3 targetRotation = Vector3.back * 360;

            //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
            prevAngle = currentAngle = wheelCircle.eulerAngles.z;

            bool isIndicatorOnTheLine = false;

            tween = wheelCircle
            .DORotate(targetRotation, spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
               float diff = Mathf.Abs(prevAngle - currentAngle);
               if (diff >= halfPieceAngle)
               {
                  if (isIndicatorOnTheLine)
                  {
                     audioSource.PlayOneShot(audioSource.clip);
                  }
                  prevAngle = currentAngle;
                  isIndicatorOnTheLine = !isIndicatorOnTheLine;
               }
               currentAngle = wheelCircle.eulerAngles.z;
            })
            .OnComplete(() =>
            {
               _isSpinning = false;
               if (onSpinEndEvent != null)
                  onSpinEndEvent.Invoke(piece);

               onSpinStartEvent = null;
               onSpinEndEvent = null;

               Spin();
            });

         }
      }

      public void Stop()
      {
         tween.Kill(false);
         _isSpinning = false;
         prevAngle = currentAngle = wheelCircle.eulerAngles.z;
      }

      public Sprite GetImage()
      {
         for (int i = 0; i < wheelPiecesList.Count; i++)
         {
            if (needle.rectTransform.Overlaps(wheelPiecesList[i], true))
               return wheelPieces[i].Icon;
         }

         return null;
      }

      private void FixedUpdate()
      {
         transformToRotate.RotateAround(wheelCircle.transform.position, new Vector3(0, 0, 1), 1f);
      }

      public void OnSpinStart(UnityAction action)
      {
         onSpinStartEvent = action;
      }

      public void OnSpinEnd(UnityAction<WheelPiece> action)
      {
         onSpinEndEvent = action;
      }


      private int GetRandomPieceIndex()
      {
         double r = rand.NextDouble() * accumulatedWeight;

         for (int i = 0; i < wheelPieces.Length; i++)
            if (wheelPieces[i]._weight >= r)
               return i;

         return 0;
      }

      private void CalculateWeightsAndIndices()
      {
         for (int i = 0; i < wheelPieces.Length; i++)
         {
            WheelPiece piece = wheelPieces[i];

            //add weights:
            accumulatedWeight += piece.Chance;
            piece._weight = accumulatedWeight;

            //add index :
            piece.Index = i;

            //save non zero chance indices:
            if (piece.Chance > 0)
               nonZeroChancesIndices.Add(i);
         }
      }




      // private void OnValidate()
      // {
      //    if (PickerWheelTransform != null)
      //       PickerWheelTransform.localScale = new Vector3(wheelSize, wheelSize, 1f);

      //    if (wheelPieces.Length > piecesMax || wheelPieces.Length < piecesMin)
      //       Debug.LogError("[ PickerWheelwheel ]  pieces length must be between " + piecesMin + " and " + piecesMax);
      // }
   }
}

public static class RectTransformExtensions
{

   public static bool Overlaps(this RectTransform a, RectTransform b)
   {
      return a.WorldRect().Overlaps(b.WorldRect());
   }
   public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
   {
      return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
   }

   public static Rect WorldRect(this RectTransform rectTransform)
   {
      Vector2 sizeDelta = rectTransform.sizeDelta;
      Vector2 pivot = rectTransform.pivot;

      float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
      float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

      //With this it works even if the pivot is not at the center
      Vector3 position = rectTransform.TransformPoint(rectTransform.rect.center);
      float x = position.x - rectTransformWidth * 0.5f;
      float y = position.y - rectTransformHeight * 0.5f;

      return new Rect(x, y, rectTransformWidth, rectTransformHeight);
   }

}