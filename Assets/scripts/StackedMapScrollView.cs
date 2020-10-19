using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.Examples;



// Class declaration
[System.Serializable]
public class UnityEventInt : UnityEvent<int> {}
public class StackedMapScrollView : MonoBehaviour
    {
        [SerializeField]
        Example04ScrollView scrollView = null;
        [SerializeField]
        Button prevCellButton = null;
        [SerializeField]
        Button nextCellButton = null;
        [SerializeField]
        Text selectedItemInfo = null;

        [SerializeField]
        public UnityEventInt OnIndexChange;
        
        void Start()
        {
            scrollView.OnSelectedIndexChanged(HandleSelectedIndexChanged);

            /*
            var cellData = Enumerable.Range(0, 3)
                .Select(i => new Example04CellDto { Message = "TWENTYFIRST CENTURY  (DATES CE) " + (15 + i) })
                .ToList();

            scrollView.UpdateData(cellData);
            scrollView.UpdateSelection(1);
            */
        }

        void HandleSelectedIndexChanged(int index)
        {
            if(selectedItemInfo!= null)
                selectedItemInfo.text = $"Selected item info: index {index}";
            
            //print(index);
            OnIndexChange.Invoke(index);
        }

        public void FocusOnIndex(int index)
        {
            scrollView.UpdateSelection(index);
        }

        public void ReGenerateUI(List<string> items)
        {
            var cellData = Enumerable.Range(0, items.Count)
                .Select(i => new Example04CellDto {Message = items[i]})
                .ToList();
            scrollView.UpdateData(cellData);
            Invoke("SelectFirstItem",0.5f);
        }
        private void SelectFirstItem()
        {
            scrollView.UpdateSelection(0);
        }
    }
