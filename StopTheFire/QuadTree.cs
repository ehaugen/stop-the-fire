using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

using StopTheFire.Extensions;

namespace StopTheFire
{
    public class QuadTree
    {
        public static int MaxActors = 50; // threshold 
        public readonly BoundingBox BoundingBox; // must be readonly, the bounding box must never change after construction 
        public List<Actor> Actors;
        public QuadTree[] nodes;

        public QuadTree(Vector3 position, Vector3 scale)
        {
            Vector3 halfScale = scale * 0.5f;
            BoundingBox = new BoundingBox(position - halfScale, position + halfScale);
            Actors = new List<Actor>(MaxActors);
        }

        public bool AddActor(Actor actor)
        {
            if (BoundingBox.Contains(actor.Position) == ContainmentType.Contains)
            {
                if (nodes == null) // we have no nodes so we are a leaf 
                {
                    if (Actors.Count < MaxActors)
                    {
                        Actors.Add(actor);
                        return true;
                    }
                    Split(); // we are a leaf and we already contain MaxActors so we need to split. this will only happen once for this node 

                    if (nodes == null) // we can't split because have run out of space, so lets just increase the threshold 
                    {
                        MaxActors *= 2;
                        return AddActor(actor);
                    }
                }
                if (!nodes[0].AddActor(actor))
                    if (!nodes[1].AddActor(actor))
                        if (!nodes[2].AddActor(actor))
                            nodes[3].AddActor(actor); // if it's not the other 3, it must be this 1. 
                return true;
            }
            return false;
        }

        public bool ActorMoved(Actor actor, QuadTree prevPosition)
        {
            if (BoundingBox.Contains(actor.Position) == ContainmentType.Contains)
            {
                if (nodes == null) // we have no nodes so we are a leaf 
                {
                    if (this != prevPosition)
                    {
                        if (prevPosition != null) prevPosition.Actors.Remove(actor);
                        AddActor(actor);
                    }
                    return true;
                }
                if (!nodes[0].ActorMoved(actor, prevPosition))
                    if (!nodes[1].ActorMoved(actor, prevPosition))
                        if (!nodes[2].ActorMoved(actor, prevPosition))
                            nodes[3].ActorMoved(actor, prevPosition); // if it's not the other 3, it must be this 1. 
                return true;
            }
            return false;
        }

        public QuadTree FindLeaf(Vector3 position)
        {
            if (BoundingBox.Contains(position) == ContainmentType.Contains)
            {
                if (nodes == null)
                {
                    return this;
                }
                else
                {
                    QuadTree result;
                    if ((result = nodes[0].FindLeaf(position)) == null)
                        if ((result = nodes[1].FindLeaf(position)) == null)
                            if ((result = nodes[2].FindLeaf(position)) == null)
                                result = nodes[3].FindLeaf(position);
                    return result;
                }
            }
            return null;
        }

        static List<QuadTree> leavesInsideFrustum = new List<QuadTree>();

        public List<QuadTree> GetLeavesInsideFrustrum(BoundingFrustum frustum)
        {
            leavesInsideFrustum.Clear();
            AddLeavesInsideFrustrum(frustum);
            return leavesInsideFrustum;
        }

        void AddLeavesInsideFrustrum(BoundingFrustum frustum)
        {
            if (nodes == null)
            {
                leavesInsideFrustum.Add(this);
            }
            else
            {
                nodes[0].AddLeavesInsideFrustrum(frustum);
                nodes[1].AddLeavesInsideFrustrum(frustum);
                nodes[2].AddLeavesInsideFrustrum(frustum);
                nodes[3].AddLeavesInsideFrustrum(frustum);
            }
        }

        void Split()
        {
            Vector3 halfScale = (BoundingBox.Max - BoundingBox.Min) * 0.5f;
            Vector3 qtrScale = halfScale * 0.5f;

            if (qtrScale != Vector3.Zero)
            {
                Vector3 topLeftPosition = BoundingBox.Min + new Vector3(qtrScale.X, 0, qtrScale.Z);
                Vector3 topRightPosition = BoundingBox.Min + new Vector3(qtrScale.X + halfScale.X, 0, qtrScale.Z);
                Vector3 bottomLeftPosition = BoundingBox.Min + new Vector3(qtrScale.X, 0, qtrScale.Z + halfScale.Z);
                Vector3 bottomRightPosition = BoundingBox.Min + new Vector3(qtrScale.X + halfScale.X, 0, qtrScale.Z + halfScale.Z);

                nodes = new QuadTree[4];
                nodes[0] = new QuadTree(topLeftPosition, halfScale);
                nodes[1] = new QuadTree(topRightPosition, halfScale);
                nodes[2] = new QuadTree(bottomLeftPosition, halfScale);
                nodes[3] = new QuadTree(bottomRightPosition, halfScale);

                ReassignActors();
                Actors.Clear();
            }
        }

        void ReassignActors()
        {
            foreach (Actor actor in Actors)
            {
                if (!nodes[0].AddActor(actor))
                    if (!nodes[1].AddActor(actor))
                        if (!nodes[2].AddActor(actor))
                            nodes[3].AddActor(actor); // if it's not the other 3, it must be this 1. 
            }
        }
    }
}