/*
 OpenGEMS is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 OpenGEMS is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

  Copyright 2007 by Computer and Communication Unlimited
*/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using GEMS.Designer.Models.GeometryModels;
using GEMS.Designer.Models;

namespace GEMS.Designer.Direct3D
{
    public class ConeRender : GeometryRender
    {
        private AutoMesh coneMesh;
        private ConeModel model;

        public ConeRender(Direct3d d3d, GeometryModel source)
            : base(d3d, source)
        {

        }

        /// <summary>
        /// Create the primitives to be drawed
        /// Z ---- length
        /// </summary>
        public override void Initialize ( )
        {
            if (coneMesh != null)
                coneMesh.Dispose();

            model = source as ConeModel;

            //Create the mesh
            coneMesh = new AutoMesh(d3d, Mesh.Cylinder(d3d.Dx, model.Radius1, model.Radius2, model.Height, 
                Direct3dRender.DefaultSlices, Direct3dRender.DefaultStacks));            
            
            //Get the bounding box
            coneMesh.BoundingBox(out minVector3, out maxVector3);            

            minVector3.TransformCoordinate(model.WorldMatrix);
            maxVector3.TransformCoordinate(model.WorldMatrix);

            model.MinVector3 = minVector3;
            model.MaxVector3 = maxVector3;
        }

        /// <summary>
        /// Draw the primitives 
        /// </summary>
        protected override void PerformRender()
        {
            d3d.Dx.Transform.World = model.WorldMatrix;
            coneMesh.M.DrawSubset(0);            
        }

        /// <summary>
        /// Clean up any resources being used. 
        /// </summary>
        protected override void PerformDispose ( )
        {
            coneMesh.Dispose ( );
            model = null;
        }    

        public override bool Intersect(Vector3 near, Vector3 far, out IntersectInformation closestHit)
        {
            Matrix inver = Matrix.Invert(model.WorldMatrix);

            near.TransformCoordinate(inver);
            far.TransformCoordinate(inver);

            return coneMesh.M.Intersect(near, far - near, out closestHit);
        }
    }
        
}
