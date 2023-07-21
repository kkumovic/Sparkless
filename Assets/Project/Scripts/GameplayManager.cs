using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sparkless.Core
{
	public class GameplayManager : MonoBehaviour
	{
        #region Start_methods
        #region Start_variables
        public static GameplayManager instance;
        [HideInInspector] public bool hasGameFinished;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private GameObject _winText;
        [SerializeField] private SpriteRenderer _clickHighlight;
        private void Awake()
        {
            instance = this;
            hasGameFinished = false;
            _winText.SetActive(false);
            _titleText.gameObject.SetActive(true);
            _titleText.text = GameManager.Instance.StageName + " - " + GameManager.Instance.CurrentLevel.ToString();

        }
        #endregion

    }
    #endregion

    #region Board_spawn

    #endregion

    #region Node_spawn

    #endregion

    #region Update_methods

    #endregion

    #region Win_condition

    #endregion

    #region Button_functions

    #endregion


}