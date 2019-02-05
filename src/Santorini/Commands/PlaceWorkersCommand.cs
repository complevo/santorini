namespace Santorini
{
    public class PlaceWorkersCommand
    {
        public Coord WorkerOne { get; set; }
        public Coord WorkerTwo { get; set; }

        public bool IsValid
            => WorkerOne.IsValid
            && WorkerTwo.IsValid
            && WorkerOne != WorkerTwo;
    }
}
