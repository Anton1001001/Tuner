namespace Tunerv1._0
{
    public class GuitarString
    {
        public float Frequency { get; set; }
        public string Note { get; set; }

        public GuitarString(float frequency, string note)
        {
            Frequency = frequency;
            Note = note;
        }
    }
}