using System;
using System.Collections.Generic;
using DrillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public class ScoreManager : MonoBehaviour
{
    public class Score
    {
        public float TotalInfectedMaterial;
        public float TotalHealthyMaterial;
        public float RemovedInfectedMaterialPercentage;
        public float RemovedHealthyMaterialPercentage;
        public int NerveTouchedCounter;
    }
    
    [Header("UI")]
    [SerializeField] private TMP_Text m_scoreboardText;
    [SerializeField] private TMP_Text timerLabel;
    
    public UnityEvent onSurgeryCompleted;
    
    private bool _timerRunning = false;
    private float _elapsedTime = 0f;
    
    private bool _surgeryCompleted = false;
    private int _totalInfectedMaterial;
    private int _totalHealthyMaterial;
    private int _removedInfectedMaterial;
    private int _removedHealthyMaterial;
    private int _nerveTouchedCounter;
    private readonly HashSet<int> _countedObjects = new();
    
    private void Start()
    {
        GetInitialValues();
        DrillController.OnPartDrilled += OnMaterialRemoved;
        DrillController.OnNerveTouched += _ => _nerveTouchedCounter++;
    }

    private void Update()
    {
        UpdateTimer();

        var score = GetScore();
        SetScoreUI(score);

        if (_surgeryCompleted) return;

        if (score.RemovedInfectedMaterialPercentage >= 100f)
        {
            _surgeryCompleted = true;
            _timerRunning = false; // Stop timer when surgery completes
            onSurgeryCompleted?.Invoke();
        }
    }

    private void UpdateTimer()
    {
        if (!_timerRunning) return;
        
        _elapsedTime += Time.deltaTime;
        var minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        var seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        
        timerLabel.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer()
    {
        _timerRunning = true;
        _elapsedTime = 0f;
        timerLabel.text = "Time: 00:00";
    }

    private void GetInitialValues()
    {
        var totalHealthyMaterial = GOUtils.FindGameObjectsWithLayer(6);
        if (totalHealthyMaterial != null)
        {
            _totalHealthyMaterial = totalHealthyMaterial.Length;
        }
        
        var totalInfectedMaterial = GOUtils.FindGameObjectsWithLayer(7);
        if (totalInfectedMaterial != null)
        {
            _totalInfectedMaterial = totalInfectedMaterial.Length;
        }
    }

    public void OnMaterialRemoved(DrillController.PieceType type, GameObject obj)
    {
        var id = obj.GetInstanceID();
        if (!_countedObjects.Add(id)) return;

        if (type == DrillController.PieceType.Healthy)
        {
            _removedHealthyMaterial += 1;
        }

        if (type == DrillController.PieceType.Infected)
        {
            _removedInfectedMaterial += 1;
        }
    }

    public Score GetScore() 
    {
        return new Score
        {
            TotalInfectedMaterial = _totalInfectedMaterial,
            TotalHealthyMaterial = _totalHealthyMaterial,
            RemovedInfectedMaterialPercentage = _totalInfectedMaterial > 0 
                ? (float)Math.Round((_removedInfectedMaterial / (float)_totalInfectedMaterial) * 100f, 2)
                : 0f,
            RemovedHealthyMaterialPercentage = _totalHealthyMaterial > 0 
                ? (float)Math.Round((_removedHealthyMaterial / (float)_totalHealthyMaterial) * 100f, 2)
                : 0f,
            NerveTouchedCounter = _nerveTouchedCounter
        };
    }

    public void SetScoreUI(Score score)
    {
        m_scoreboardText.text = $"Carries removed:\t\t\t{score.RemovedInfectedMaterialPercentage}%\n"+
        $"Healthy removed:\t\t\t{score.RemovedHealthyMaterialPercentage}%\n" +
        $"Nerve touched counter:\t\t\t{score.NerveTouchedCounter}";
    }
    
    private void OnDestroy()
    {
        DrillController.OnPartDrilled -= OnMaterialRemoved;
    }
}
