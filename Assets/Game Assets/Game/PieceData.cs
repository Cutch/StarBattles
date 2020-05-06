using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using StarBattles;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System;

namespace StarBattles
{
    [Serializable()]
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
        internal int version = 1;

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
        public override string ToString()
        {
            string name = DB.getObjectById(shipObjectId).name;
            
            return "name: " + name + "\n" +
             "objectId: " + objectId + "\n" +
             "shipObjectId: " + shipObjectId + "\n" +
             "location: " + location + "\n" +
             "rotation: " + rotation + "\n" +
             "size: " + size + "\n" +
             "joinedPieceids: " + string.Join(", ", joinedPieceids) + "\n" +
             "joinedPointIds: " + string.Join(", ", joinedPointIds) + "\n" +
             "joinedPieceidsL: " + joinedPieceids.Length + "\n" +
             "joinedPointIdsL: " + joinedPointIds.Length + "\n" +
             "saveId: " + saveId + "\n";
        }
        void arrayPad<T>(ref T[] a, int size)
        {
            if(a.Length == size)
            {
                return;
            }
            Array.Resize(ref a, size);
        }
        void v1Align()
        {
            DB.ShipBean sb = DB.getObjectById(shipObjectId);
            arrayPad(ref this.joinedPieceids, sb.mountPoints.Length);
            arrayPad(ref this.joinedPointIds, sb.mountPoints.Length);
        }
        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            v1Align();
        }
    }
}