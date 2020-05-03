using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using StarBattles;
namespace StarBattles
{
    [System.Serializable]
    public class PieceData
    {
        internal int shipObjectId;
        internal char[] fireKeys;
        internal Vector2 location;
        internal Vector3 rotation;
        internal Vector2 size;
        internal int[] joinedPieceids;
        internal int[] joinedPointIds;
        internal int saveId;
        internal int objectId;


        public PieceData(EditorPiece ep)
        {
            this.location = (Vector2)ep.gameObject.transform.position;
            this.rotation = ep.gameObject.transform.rotation.eulerAngles;
            this.size = ep.gameObject.GetComponent<RectTransform>().rect.size;
            this.fireKeys = ep.getFireKey();
            this.saveId = ep.getSaveId();
            this.objectId = ep.getId();
            this.shipObjectId = ep.shipObjectId;
            this.joinedPieceids = new int[ep.mountPoints.Length];
            for (int i = 0; i < ep.mountPoints.Length; i++)
            {
                if (ep.getJoinedObjects()[i])
                    this.joinedPieceids[i] = ep.getJoinedObjects()[i].getSaveId();
            }
            this.joinedPointIds = new int[ep.mountPoints.Length];
            JoinPoint[] jps = ep.getMountPointObjects();
            for (int i = 0; i < ep.mountPoints.Length; i++)
            {
                if (jps[i].isJoined())
                {
                    this.joinedPointIds[i] = jps[i].getOtherJoinId();
                }
            }
        }

    }
}