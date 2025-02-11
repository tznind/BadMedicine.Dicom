﻿using FellowOakDicom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BadMedicine.Dicom
{
    /// <summary>
    /// Data class representing a single dicom series.  Stores
    /// the DICOM tags that fit at the series level hierarchy
    /// (and are modelled by BadMedicine.Dicom).
    /// </summary>
    public class Series : IEnumerable<DicomDataset>
    {
        /// <summary>
        /// The unique identifier for this series
        /// </summary>
        public DicomUID SeriesUID {get; }
        
        /// <summary>
        /// The Dicom Study this series is a part of
        /// </summary>
        public Study Study{get; }

        /// <summary>
        /// All dicom images generated for this series.  These can be
        /// written out to file by other processes and do not yet exist
        /// on disk.
        /// </summary>
        public IReadOnlyList<DicomDataset> Datasets{get; }

        private readonly List<DicomDataset> _datasets = new();
        
        /// <summary>
        /// Patient level information generated by BadMedicine.Dicom for whom
        /// the study exists.
        /// </summary>
        public Person person;


        /// <summary>
        /// Value to use for the <see cref="DicomTag.Modality"/> when writting
        /// out to dicom datasets
        /// </summary>
        public string Modality {get; }

        /// <summary>
        /// Value to use for the <see cref="DicomTag.ImageType"/> when writting
        /// out to dicom datasets
        /// </summary>
        public string ImageType {get; }

        /// <summary>
        /// Date to use for the <see cref="DicomTag.SeriesDate"/> when writting
        /// out to dicom datasets
        /// </summary>
        public DateTime SeriesDate { get; internal set; }

        /// <summary>
        /// Value to use for the <see cref="DicomTag.SeriesTime"/> when writting
        /// out to dicom datasets
        /// </summary>
        public TimeSpan SeriesTime { get; internal set; }


        /// <summary>
        /// Value to use for the <see cref="DicomTag.NumberOfSeriesRelatedInstances"/> when writting
        /// out to dicom datasets
        /// </summary>
        public int NumberOfSeriesRelatedInstances { get; }


        /// <summary>
        /// Value to use for the <see cref="DicomTag.SeriesDescription"/> when writting
        /// out to dicom datasets
        /// </summary>
        public string SeriesDescription { get; }


        /// <summary>
        /// Value to use for the <see cref="DicomTag.BodyPartExamined"/> when writting
        /// out to dicom datasets
        /// </summary>
        public string BodyPartExamined { get; }

        internal Series(Study study, Person person, string modality, string imageType, int imageCount, DescBodyPart part = null)
        {
            SeriesUID = UIDAllocator.GenerateSeriesInstanceUID();

            this.Study = study;
            this.person = person;
            this.Modality = modality;
            this.ImageType = imageType;
            this.NumberOfSeriesRelatedInstances = imageCount;
            
            //todo: for now just use the Study date, in theory secondary capture images could be generated later
            SeriesDate = study.StudyDate;
            SeriesTime = study.StudyTime;

            if(part != null)
            {
                SeriesDescription =  part.SeriesDescription;
                BodyPartExamined = part.BodyPartExamined;
            }
            

            for (int i =0 ; i<imageCount;i++)
                _datasets.Add(Study.Parent.GenerateTestDataset(person,this));
            
            Datasets = new ReadOnlyCollection<DicomDataset>(_datasets);
        }

        
        /// <summary>
        /// Returns <see cref="Datasets"/> as IEnumerable
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DicomDataset> GetEnumerator()
        {
            return _datasets.GetEnumerator();
        }

        /// <summary>
        /// Returns <see cref="Datasets"/> as IEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _datasets.GetEnumerator();
        }
    }
}
