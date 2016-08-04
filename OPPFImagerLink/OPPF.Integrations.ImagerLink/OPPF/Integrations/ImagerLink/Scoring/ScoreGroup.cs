using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formulatrix.Integrations.ImagerLink.Scoring;

namespace OPPF.Integrations.ImagerLink.OPPF.Integrations.ImagerLink.Scoring
{
    class ScoreGroup : IScoreGroup
    {
        private int _iD;

        private string _name;

        # region Setters

        /// <summary>
        /// Setter for IScoreGroup.ID read-only property
        /// </summary>
        /// <param name="iD"></param>
        public void setID(int iD)
        {
            _iD = iD;
        }

        /// <summary>
        /// Setter for IScoreGroup.Name read-only property
        /// </summary>
        /// <param name="name"></param>
        public void setName(string name)
        {
            _name = name;
        }

        # endregion

        # region IScoreGroup properties

        /// <summary>
        /// Undocumented ImagerLink API read-only property IScoreGroup.ID
        /// </summary>
        int IScoreGroup.ID
        {
            get
            {
                return _iD;
            }
        }

        /// <summary>
        /// Undocumented ImagerLink API read-only property IScoreGroup.Name
        /// </summary>
        string IScoreGroup.Name
        {
            get
            {
                return _name;
            }
        }

        # endregion

    }
}
