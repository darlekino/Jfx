using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public class JfxCamera
    {
        private JfxVector3F position;
        private JfxVector3F target;
        private JfxVector3F upVector;
        private JfxMatrix4F transformation;

        public ref readonly JfxVector3F Position => ref position;
        public ref readonly JfxVector3F Target => ref target;
        public ref readonly JfxVector3F UpVector => ref upVector;
        public ref readonly JfxMatrix4F Transformation => ref transformation;
        public event EventHandler Changed;

        public JfxCamera(in JfxVector3F position, in JfxVector3F target, in JfxVector3F upVector)
        {
            this.position = position;
            this.target = target;
            this.upVector = upVector;
            UpdateTransformation();
        }

        private void UpdateTransformation()
        {
            transformation = JfxMatrix4F.LookAtRH(position, target, upVector);
        }

        private void UpdateTransformationAndInvokeChanged()
        {
            UpdateTransformation();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void MoveTo(in JfxVector3F position)
        {
            this.position = position;
            UpdateTransformationAndInvokeChanged();
        }

        public void LookAt(in JfxVector3F target)
        {
            this.target = target;
            UpdateTransformationAndInvokeChanged();
        }

        public void Rotate(in JfxVector3F upVector)
        {
            this.upVector = upVector;
            UpdateTransformationAndInvokeChanged();
        }

        public void Update(in JfxVector3F position, in JfxVector3F target, in JfxVector3F upVector)
        {
            this.position = position;
            this.target = target;
            this.upVector = upVector;
            UpdateTransformationAndInvokeChanged();
        }
    }
}
