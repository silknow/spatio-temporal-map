using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.Examples;

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

        void Start()
        {
           
            scrollView.OnSelectedIndexChanged(HandleSelectedIndexChanged);

            var cellData = Enumerable.Range(0, 4)
                .Select(i => new Example04CellDto { Message = "Siglo " + (15 + i) })
                .ToList();

            scrollView.UpdateData(cellData);
            scrollView.UpdateSelection(1);
        }

        void HandleSelectedIndexChanged(int index)
        {
            if(selectedItemInfo!= null)
                selectedItemInfo.text = String.Format("Selected item info: index {0}", index);
        }
    }
