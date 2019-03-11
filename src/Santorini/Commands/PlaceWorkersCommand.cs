using System.Runtime.Serialization;

namespace Santorini
{
    public class PlaceWorkersCommand
    {
        public Coord WorkerOne { get; set; }
        public Coord WorkerTwo { get; set; }

        [IgnoreDataMember]
        public bool IsValid
            => WorkerOne.IsValid
            && WorkerTwo.IsValid
            && WorkerOne != WorkerTwo;
    }
}
