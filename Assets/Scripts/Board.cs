using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{

    public Tilemap tilemap { get; private set; }
    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileNumEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;
    public Tile tileRock;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(CellGrid grid)
    {
        int width = grid.Width;
        int height = grid.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if(!cell.block)
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    public void InitiateDraw(CellGrid grid, int rockCount) 
    {
        int width = grid.Width;
        int height = grid.Height;

        //Tao rock
        for (int i = 0; i < rockCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            Cell cell = grid[x, y];
            tilemap.SetTile(cell.position, tileRock);
            cell.block = true;
            cell.revealed = true;
            cell.type = Cell.Type.Block;
        }

        //Tao hinh dang ngau nhien cho map bang cach xoa bot tile
        int[] edgesx = { 0, width - 1 };
        foreach (int x in edgesx)             // Xử lý cột đầu và cuối
        {
            for (int y = 0; y < height; y++)
            {
                if (y == 1 || y == 2 || y == height - 2 || x == height - 3) //Khong xoa vi tri o gan goc
                { continue; }
                Cell cell = grid[x, y];
                if (Random.value > 0.5f) // Xác suất xóa thêm ngẫu nhiên
                {
                   ClearTile(cell);

                    Cell cell2 = grid[(x == 0) ? (x + 1) : (x - 1), y];
                    if (Random.value > 0.6f)          //Xac suat xoa sau vao them
                    {                      
                        ClearTile(cell2);

                        Cell cell3 = grid[(x == 0) ? (x + 2) : (x - 2), y];
                        if (!cell.block && Random.value > 0.8f)          //Xac suat xoa sau vao them nua
                        {                   
                            ClearTile(cell3);
                        }
                    }
                }
            }
        }
        int[] edgesy = { 0, height - 1 };
        foreach (int y in edgesy)  // Xử lý hàng đầu và cuối
        {
            for (int x = 0; x < width; x++)
            {
                if(x == 1 || x == 2 || x == width - 2|| x == width - 3)  //Khong xoa vi tri o gan goc
                { continue; }
                Cell cell = grid[x, y];
                if (Random.value > 0.5f) // Xác suất xóa thêm ngẫu nhiên
                {
                    ClearTile(cell);

                    Cell cell2 = grid[x, (y == 0) ? (y + 1) : (y - 1)];
                    if (Random.value > 0.6f)          //Xac suat xoa sau vao them
                    {                   
                        ClearTile(cell2);

                        Cell cell3 = grid[x, (y == 0) ? (y + 2) : (y - 2)];
                        if (Random.value > 0.8f)          //Xac suat xoa sau vao them nua
                        {
                            ClearTile(cell3);
                        }
                    }
                }
            }
        }

    }
    
    private void ClearTile(Cell cell)
    {
        tilemap.SetTile(cell.position, null);  // Xoa tile
        cell.type = Cell.Type.Empty;          // Xem như tile rỗng
        cell.revealed = true;                // Không thể đào
        cell.block = true;                  // Không thể spawn mine
    }

    private Tile GetTile(Cell cell)

    {
        if (cell.block)
        {
            return tileRock;
        }    
        else if (cell.numempty)
        {
            return tileEmpty;
        }
        else if (cell.revealed) {
            return GetRevealedTile(cell);
        } else if (cell.flagged) {
            return tileFlag;
        } 
        else {
            return tileUnknown;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }

}
