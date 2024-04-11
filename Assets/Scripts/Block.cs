using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool isSelected = false;
    private bool isDragging = false;
    //private Vector3 offset;
    
    private SpriteRenderer spriteRenderer;
    private BlockManager blockManager;
    private Vector3 initialPos;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;
    private float padding = 1f;
    private float gridCellSize = 1f;
    private float gridXMin = -2f;
    private float gridXMax = 2f;
    private float gridYMin = -2f;
    private float gridYMax = 2f;
    private float gridSnapOffset = 0.5f;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        blockManager = FindObjectOfType<BlockManager>();

        SetUpMoveBoundries();
    }

    private void OnMouseDown(){
        Debug.Log("Entered OnMouseDown");
        if (!isDragging){
            isSelected = false;
            blockManager.UpdateBlockSelection(this, isSelected);
            isDragging = true;
            Debug.Log("isDragging set to true");
            initialPos = transform.position;
            //var mouseWorldPos = GetMouseWorldPosition();
            //offset = transform.position - new Vector3 (mouseWorldPos.x, mouseWorldPos.y, 0f);
        }
    }

    private void OnMouseDrag(){
        if(isDragging){
            var currentMouseWorldPos = GetMouseWorldPosition();
            var newPosX = Mathf.Clamp(currentMouseWorldPos.x, xMin, xMax);
            var newPosY = Mathf.Clamp(currentMouseWorldPos.y, yMin, yMax);

            if(IsOnGrid()){                    
                transform.position = SnapPositionToGrid(new Vector3 (newPosX, newPosY, -1f));
            }
            else{
                transform.position = new Vector3(newPosX, newPosY, -1f);
            }

            if(IsPositionTaken(transform.position)){
                spriteRenderer.color = Color.red;            
            }
            else{
                spriteRenderer.color = Color.white;
            }
            
            //transform.position = GetMouseWorldPosition() + offset;

        }
    }

    private void OnMouseUp(){
        isDragging = false;
        if(!IsOnGrid() || IsPositionTaken(transform.position)){
            transform.position = initialPos;
        }
        else{
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }

        spriteRenderer.color = Color.white;
        
        Debug.Log("isDragging set to false");
    }

    private Vector3 GetMouseWorldPosition(){
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private bool IsOnGrid(){
        if(transform.position.x >= gridXMin && transform.position.x <= gridXMax &&
            transform.position.y >= gridYMin && transform.position.y <= gridYMax){
                return true;
            }
        return false;
    }

    private Vector3 SnapPositionToGrid(Vector3 position){
        float snappedX = Mathf.Floor(position.x / gridCellSize) * gridCellSize;
        float snappedY = Mathf.Floor(position.y / gridCellSize) * gridCellSize;
        return new Vector3(snappedX + gridSnapOffset, snappedY + gridSnapOffset, position.z);
    }

    private bool IsPositionTaken(Vector3 position){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders){
            if (collider.gameObject != gameObject){
                return true;
            }
        }
        return false;
    }

    // change so padding is set based on block type
    private void SetUpMoveBoundries(){
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    public void ToggleSelection(bool isMaxCount){
        if(!isSelected && IsOnGrid() && !isMaxCount){
            isSelected = true;
            blockManager.UpdateBlockSelection(this, isSelected);
            spriteRenderer.color = Color.green;
        }
        else{
            isSelected = false;
            blockManager.UpdateBlockSelection(this, isSelected);
            spriteRenderer.color = Color.white;
        }
    }
}
