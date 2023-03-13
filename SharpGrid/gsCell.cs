using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGrid
{
    public class gsCell : System.Collections.CollectionBase
    {
        public Color cellColor;
        public Bitmap cellBitmap = null;
        public Point cellPoint;

        public gsCell()
        {

        }

        public gsCell(Point cellPnt, Color cellClr)
        {
            cellPoint = cellPnt;
            cellColor = cellClr;
        }

        public gsCell(Point cellPnt, Bitmap btmp)
        {
            cellPoint = cellPnt;
            cellBitmap = btmp;
        }


        public void Add(Point cellPnt, Color cellClr)
        {
            List.Add(new gsCell(cellPnt, cellClr));
        }

        public void Add(Point cellPnt, Bitmap btmp)
        {
            List.Add(new gsCell(cellPnt, btmp));
        }

        public gsCell this[int num]
        {
            get { return List[num] as gsCell; }
            set { List[num] = value as gsCell; }
        }

        public void Remove(Point cellPnt)
        {
            for (int i = 0; i < List.Count; i++)
            {
                gsCell tempCell = List[i] as gsCell;
                if (tempCell.cellPoint == cellPnt)
                {
                    if (tempCell.cellBitmap != null)
                    {
                        tempCell.cellBitmap.Dispose();
                        tempCell.cellBitmap = null;
                    }
                    List.RemoveAt(i);
                }
            }
        }

        protected override void OnClear()
        {
            for (int i = 0; i < List.Count; i++)
            {
                gsCell tempCell = List[i] as gsCell;
                if (tempCell.cellBitmap != null)
                {
                    tempCell.cellBitmap.Dispose();
                    tempCell.cellBitmap = null;
                }
                List.RemoveAt(i);
            }
            //base.OnClear();
        }
    }
}
