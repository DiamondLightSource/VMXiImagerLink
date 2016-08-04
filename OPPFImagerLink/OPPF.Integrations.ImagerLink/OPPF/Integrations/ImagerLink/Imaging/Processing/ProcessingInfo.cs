using System;

using Formulatrix.Integrations.ImagerLink.Imaging;
using Formulatrix.Integrations.ImagerLink.Imaging.Processing;

namespace OPPF.Integrations.ImagerLink.Imaging.Processing
{
	/// <summary>
	/// Class to enable the description of an imaging for processing.
	/// </summary>
	/// <remarks>The interface is completely undocumented in the ImagerLink code.</remarks>
	public class ProcessingInfo : IProcessingInfo
	{
		public RegionType _regionType;
		public string _profileID;
		public string _regionID;
		public string _plateID;
		public int _dropNumber;
		public int _wellNumber;
		public string _imagingID;

		public ProcessingInfo()
		{
			SetRegionType(RegionType.Overview);
			SetProfileID("");
			SetRegionID("");
			SetPlateID("");
			SetDropNumber(0);
			SetWellNumber(0);
			SetImagingID("");
		}

		#region IProcessingInfo Members

        public RegionType RegionType
		{
			get
			{
				return _regionType;
			}
		}

        public string ProfileID
		{
			get
			{
				return _profileID;
			}
		}

        public string RegionID
		{
			get
			{
				return _regionID;
			}
		}

        public string PlateID
		{
			get
			{
				return _plateID;
			}
		}

        public int DropNumber
		{
			get
			{
				return _dropNumber;
			}
		}

        public int WellNumber
		{
			get
			{
				return _wellNumber;
			}
		}

        public string ImagingID
		{
			get
			{
				return _imagingID;
			}
		}

		#endregion

        #region Set methods for interface properties

        public void SetRegionType(RegionType regionType)
        {
            _regionType = regionType;
        }

        public void SetProfileID(string profileID)
        {
            _profileID = profileID;
        }

        public void SetRegionID(string regionID)
        {
            _regionID = regionID;
        }

        public void SetPlateID(string plateID)
        {
            _plateID = plateID;
        }

        public void SetDropNumber(int dropNumber)
        {
            _dropNumber = dropNumber;
        }

        public void SetWellNumber(int wellNumber)
        {
            _wellNumber = wellNumber;
        }

        public void SetImagingID(string imagingID)
        {
            _imagingID = imagingID;
        }

        #endregion

    }
}
