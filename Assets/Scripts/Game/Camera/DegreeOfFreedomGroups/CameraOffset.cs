namespace Benco.Camera
{
    public enum CameraOffsetPriority
    {
        NoOp,
        Size,
    }

    public class CameraOffsetPriorities
    {
        private int[] _values = null;
        public int[] values
        {
            get
            {
                if (_values == null)
                {
                    _values = new int[(int)CameraOffsetPriority.Size];
                    int cur = 0;
                    for (CameraOffsetPriority i = 0; i < CameraOffsetPriority.Size; i++)
                    {
                        _values[cur] = (int)i;
                        cur++;
                    }
                }
                return _values;
            }
        }

    }
}
