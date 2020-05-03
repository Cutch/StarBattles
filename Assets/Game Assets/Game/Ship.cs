using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace StarBattles
{
    [System.Serializable]
    class Ship
    {
        public string name;
        public PieceData[] piecesData;
        public Vector2 centroid;
        public bool allConnected;
        public bool allLinked;

        //private Piece[] pieces;
        public Ship(string name, List<EditorPiece> editorPieces)
        {
            this.name = name;
            int i = 1;
            foreach (EditorPiece ep in editorPieces)
            {
                ep.setSaveId(i++);
            }
            this.piecesData = editorPieces.Select(x => new PieceData(x)).ToArray();
            if (this.piecesData.Count() > 0)
                setCenter();
            //this.pieces = piecesData.Select(x => new Piece(x)).ToArray();
        }
        void setCenter()
        {
            float minX = float.NaN, minY = float.NaN, maxX = float.NaN, maxY = float.NaN;
            foreach (PieceData pd in this.piecesData)
            {
                minX = (!float.IsNaN(minX) ? Math.Min(pd.location.x, minX) : pd.location.x);
                minY = (!float.IsNaN(minY) ? Math.Min(pd.location.y, minY) : pd.location.y);
                maxX = (!float.IsNaN(maxX) ? Math.Max(pd.location.x, maxX) : pd.location.x);
                maxY = (!float.IsNaN(maxY) ? Math.Max(pd.location.y, maxY) : pd.location.y);
            }
            centroid = new Vector2((maxX - minX) / 2, (maxY - minY) / 2);
            foreach (PieceData pd in this.piecesData)
            {
                pd.location.x -= minX + centroid.x;
                pd.location.y -= minY + centroid.y;
            }
        }
    }
}