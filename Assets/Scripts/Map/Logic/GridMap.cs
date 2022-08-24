using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    public Tilemap currentTilemap;

    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();

            if (mapData !=null)
            {
                mapData.tileProperties.Clear();
            }

        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            UpdateTilemapProperties();

#if UNITY_EDITOR
            if (mapData != null)
            {
                EditorUtility.SetDirty(mapData);
            }
#endif
        }
    }

    private void UpdateTilemapProperties()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this))
        {
            Vector3Int startPos = currentTilemap.cellBounds.min;
            Vector3Int endPos = currentTilemap.cellBounds.max;

            for (int x = startPos.x; x < endPos.x; x++)
            {
                for (int y = startPos.y; y < endPos.y; y++)
                {
                    TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));

                    if (tile!=null)
                    {
                        TileProperty newTile = new TileProperty
                        {
                            tileCoordinate = new Vector2Int(x, y),
                            gridType = this.gridType,
                            boolTypeValue = true,
                        };

                        mapData.tileProperties.Add(newTile);
                    }

                }
            }
        }
    }
}
