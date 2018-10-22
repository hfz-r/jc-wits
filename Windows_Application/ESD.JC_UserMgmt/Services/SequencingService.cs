﻿using ESD.JC_UserMgmt.ModelsExt;
using ESD.JC_UserMgmt.Utilities;
using System.Collections.ObjectModel;
using System.Linq;

namespace ESD.JC_UserMgmt.Services
{
    public static class SequencingService
    {
        public static ObservableCollection<T> SetCollectionSequence<T>(ObservableCollection<T> targetCollection) where T : ISequencedObject
        {
            var sequenceNumber = 1;
            var CurrentDateTime = System.DateTime.Now;

            foreach (ISequencedObject sequencedObject in targetCollection)
            {
                sequencedObject.SequenceNumber = sequenceNumber;

                if (sequencedObject.ID == 0)
                    sequencedObject.ModifiedOn = CurrentDateTime;

                sequenceNumber++;
            }

            return targetCollection;
        }
    }
}