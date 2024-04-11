using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BlockManager : MonoBehaviour
{

    private bool hasClicked = false;
    private float singleBlockSize = 1f;
    private int maxSelectedBlocks = 2;
    private List<Block> selectedBlocks = new();

    void Update()
    {
        if(Input.GetMouseButtonDown(1) && !hasClicked){
            RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
            
            if(hit.collider != null){
                Block block = hit.collider.GetComponent<Block>();

                if(block != null){
                    block.ToggleSelection(selectedBlocks.Count == maxSelectedBlocks);
                    hasClicked = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(1)){
            hasClicked = false;
        }

        if (Input.GetMouseButtonDown(2)){
            CombineBlocks();
        }        
    }

    public void UpdateBlockSelection(Block block, bool value){
        if(value){
            selectedBlocks.Add(block);
        }
        else{
            selectedBlocks.Remove(block);
        }
        Debug.Log("# of currently selected blocks: " + selectedBlocks.Count);
    }

    public void CombineBlocks(){
        if(selectedBlocks.Count == maxSelectedBlocks){
            if(CheckBlocksAdjacent(selectedBlocks[0], selectedBlocks[1])){  

                Vector3 combinedBlockSize = CalculateCombinedBlockSize();
                Vector3 combinedBlockPosition = CalculateCombinedBlockPosition();

                GameObject combinedBlock = new GameObject("CombinedBlock");
                combinedBlock.transform.position = combinedBlockPosition;

                BoxCollider2D collider = combinedBlock.AddComponent<BoxCollider2D>();
                collider.size = combinedBlockSize;

                SpriteRenderer renderer = combinedBlock.AddComponent<SpriteRenderer>();
                renderer.color = Color.white;

                foreach(Block block in selectedBlocks){
                    Destroy(block.gameObject);
                }

                selectedBlocks.Clear();
            }

            else{
                Debug.Log("Selected blocks are not adjacent.");
            }
        }
        else{
            Debug.Log(maxSelectedBlocks + " need to be selected.");
        }
    }

    private bool CheckBlocksAdjacent(Block block1, Block block2){
        bool isAdjacent = false;

        float deltaX = Math.Abs(block1.transform.position.x - block2.transform.position.x);
        float deltaY = Math.Abs(block1.transform.position.y - block2.transform.position.y);

        Debug.Log("deltaX is " + deltaX);
        Debug.Log("deltaY is " + deltaY);

        if(deltaX == singleBlockSize && deltaY == 0f){
            isAdjacent = true;
        }
        else if(deltaX == 0f && deltaY == singleBlockSize){
            isAdjacent = true;
        }

        return isAdjacent;
    }

    private Vector3 GetMouseWorldPosition(){
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector3 CalculateCombinedBlockSize()
    {
        Vector3 combinedBlockSize = Vector3.zero;
        foreach (Block block in selectedBlocks){
            Vector2 blockColliderSize = block.GetComponent<BoxCollider2D>().size;
            combinedBlockSize += new Vector3(blockColliderSize.x, blockColliderSize.y, 0f);
        }
        return combinedBlockSize;
    }

    private Vector3 CalculateCombinedBlockPosition()
    {
        Vector3 combinedBlockPosition = Vector3.zero;
        foreach (Block block in selectedBlocks)
        {
            combinedBlockPosition += block.transform.position;
        }
        combinedBlockPosition /= selectedBlocks.Count;
        return combinedBlockPosition;
    }
}
