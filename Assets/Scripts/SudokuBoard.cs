using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SudokuBoard : MonoBehaviour
{
    [Header("Setup Variables")]
    [SerializeField] int[] row1 = new int[9];
    [SerializeField] int[] row2 = new int[9];
    [SerializeField] int[] row3 = new int[9];
    [SerializeField] int[] row4 = new int[9];
    [SerializeField] int[] row5 = new int[9];
    [SerializeField] int[] row6 = new int[9];
    [SerializeField] int[] row7 = new int[9];
    [SerializeField] int[] row8 = new int[9];
    [SerializeField] int[] row9 = new int[9];
    [SerializeField] GameObject row1Text, row2Text, row3Text, row4Text, row5Text, row6Text, row7Text, row8Text, row9Text;
    [SerializeField] GameObject currentTile;
    [SerializeField] GameObject scanningTiles;
 
    int[,] boardNumbers = new int[9, 9];
    TextMeshProUGUI[,] boardText = new TextMeshProUGUI[9, 9];
    List<int> possibleSolutions = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    [Header("Tracking Variables")]
    [SerializeField] List<int> currentPossibleSolutions = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    [SerializeField] int iterationLimit = 0;
    [SerializeField] int currentArea = 0;
    [SerializeField] bool isSearhingRow = false;
    [SerializeField] bool isSearchingColumn = false;
    [SerializeField] bool isSearchingArea = false;
    [SerializeField] Image rowImage;
    [SerializeField] Image columnImage;
    [SerializeField] Image areaImage;

    bool isSolvingPuzzle = true;

    [Header("Solver Control")]
    [SerializeField] float timeToWait = .5f;

    private void Start()
    {
        SetupBoard();
        SetupIndicators();
        StartCoroutine(SolvePuzzle());
    }

    IEnumerator SolvePuzzle()
    {
        while (isSolvingPuzzle)
        {
            iterationLimit++;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (boardNumbers[i, j] != 0)
                    {
                        continue;
                    }
                    yield return new WaitForSeconds(timeToWait);
                    if (isSearchingArea)
                    {
                        AssignCurrentArea(j, i);
                    }
                    if (isSearchingColumn)
                    {
                        EvaluateColumn(j);
                    }
                    if (isSearhingRow)
                    {
                        EvaluateRow(i);
                    }
                    

                    if (currentPossibleSolutions.Count == 1)
                    {
                        GameObject centerTile = Instantiate(currentTile, this.transform.parent.transform);
                        centerTile.transform.position = boardText[i, j].transform.position;
                        Destroy(centerTile, timeToWait);

                        boardText[i, j].text = currentPossibleSolutions[0].ToString();
                        boardText[i, j].color = Color.red;
                        boardNumbers[i, j] = currentPossibleSolutions[0];
                        currentPossibleSolutions = new List<int>(possibleSolutions);
                    }
                    else
                    {
                        GameObject centerTile = Instantiate(currentTile, this.transform.parent.transform);
                        centerTile.transform.position = boardText[i, j].transform.position;
                        Destroy(centerTile, timeToWait);

                        currentPossibleSolutions = new List<int>(possibleSolutions);
                    }
                }
            }
            if (iterationLimit == 100)
            {
                isSolvingPuzzle = false;
            }
        }

    }
    void EvaluateArea()
    {
        int rowBuffer = 0;
        int columnBuffer = 0;
        switch (currentArea)
        {
            case 1:
            case 4:
            case 7:
                rowBuffer = 0;
                break;
            case 2:
            case 5:
            case 8:
                rowBuffer = 3;
                break;
            case 3:
            case 6:
            case 9:
                rowBuffer = 6;
                break;
        }
        switch (currentArea)
        {
            case 1:
            case 2:
            case 3:
                columnBuffer = 0;
                break;
            case 4:
            case 5:
            case 6:
                columnBuffer = 3;
                break;
            case 7:
            case 8:
            case 9:
                columnBuffer = 6;
                break;
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject scanTile = Instantiate(scanningTiles, this.transform.parent.transform);
                scanTile.transform.position = boardText[i+columnBuffer,j+rowBuffer].transform.position;
                Destroy(scanTile, timeToWait);
                for (int k = 0; k < currentPossibleSolutions.Count; k++)
                {
                    if (boardNumbers[i + columnBuffer, j + rowBuffer] == currentPossibleSolutions[k])
                    {
                        currentPossibleSolutions.RemoveAt(k);
                        continue;
                    }
                }
            }
        }
    }

    void AssignCurrentArea(int row, int column)
    {
        if (row < 3 && column < 3)
        {
            currentArea = 1;
        }
        else if (row < 6 && row >= 3 && column < 3)
        {
            currentArea = 2;
        }
        else if (row >= 6 && column < 3)
        {
            currentArea = 3;
        }
        if (row < 3 && column < 6 && column >= 3)
        {
            currentArea = 4;
        }
        else if (row < 6 && row >= 3 && column < 6 && column >= 3)
        {
            currentArea = 5;
        }
        else if (row >= 6 && column < 6 && column >= 3)
        {
            currentArea = 6;
        }
        if (row < 3 && column >= 6)
        {
            currentArea = 7;
        }
        else if (row < 6 && row >= 3 && column >= 6)
        {
            currentArea = 8;
        }
        else if (row >= 6 && column >= 6)
        {
            currentArea = 9;
        }
        EvaluateArea();
    }

    void EvaluateRow(int column)
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject scanTile = Instantiate(scanningTiles, this.transform.parent.transform);
            scanTile.transform.position = boardText[column, i].transform.position;
            Destroy(scanTile, timeToWait);
            for (int j = 0; j < currentPossibleSolutions.Count; j++)
            {
                if (boardNumbers[column, i] == currentPossibleSolutions[j])
                {
                    currentPossibleSolutions.RemoveAt(j);
                    continue;
                }
            }
        }
    }

    void EvaluateColumn(int row)
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject scanTile = Instantiate(scanningTiles, this.transform.parent.transform);
            scanTile.transform.position = boardText[i, row].transform.position;
            Destroy(scanTile, timeToWait);
            for (int j = 0; j < currentPossibleSolutions.Count; j++)
            {
                if (boardNumbers[i, row] == currentPossibleSolutions[j])
                {
                    currentPossibleSolutions.RemoveAt(j);
                    continue;
                }
            }
        }
    }

    private void SetupBoard()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                switch (i)
                {
                    case 0:
                        boardNumbers[i, j] = row1[j];
                        boardText[i, j] = row1Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 1:
                        boardNumbers[i, j] = row2[j];
                        boardText[i, j] = row2Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 2:
                        boardNumbers[i, j] = row3[j];
                        boardText[i, j] = row3Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 3:
                        boardNumbers[i, j] = row4[j];
                        boardText[i, j] = row4Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 4:
                        boardNumbers[i, j] = row5[j];
                        boardText[i, j] = row5Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 5:
                        boardNumbers[i, j] = row6[j];
                        boardText[i, j] = row6Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 6:
                        boardNumbers[i, j] = row7[j];
                        boardText[i, j] = row7Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 7:
                        boardNumbers[i, j] = row8[j];
                        boardText[i, j] = row8Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    case 8:
                        boardNumbers[i, j] = row9[j];
                        boardText[i, j] = row9Text.GetComponentsInChildren<TextMeshProUGUI>()[j];
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void SetupIndicators()
    {
        if (isSearhingRow)
        {
            rowImage.color = Color.green;
        }
        if (isSearchingColumn)
        {
            columnImage.color = Color.green;
        }
        if (isSearchingArea)
        {
            areaImage.color = Color.green;
        }
    }
}
