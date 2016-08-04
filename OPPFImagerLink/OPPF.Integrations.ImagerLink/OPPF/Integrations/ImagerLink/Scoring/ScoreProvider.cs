using Formulatrix.Integrations.ImagerLink.Scoring;

namespace OPPF.Integrations.ImagerLink.OPPF.Integrations.ImagerLink.Scoring
{
    /// <summary>
    /// Do-nothing implementation of undocumented ImagerLink API interface IScoreProvider
    /// </summary>
    class ScoreProvider : IScoreProvider
    {
        # region IScoreProvider methods

        /// <summary>
        /// Undocumented ImagerLink API method GetScore
        /// </summary>
        /// <param name="imageBatchID"></param>
        /// <param name="plateID"></param>
        /// <param name="wellNumber"></param>
        /// <param name="dropNumber"></param>
        /// <param name="wellDropScoreGroupID"></param>
        /// <returns></returns>
        double IScoreProvider.GetScore(int imageBatchID, string plateID, int wellNumber, int dropNumber, int wellDropScoreGroupID)
        {
            return 0d;
        }

        /// <summary>
        /// Undocumented ImagerLink API method GetScoreGroups
        /// </summary>
        /// <returns></returns>
        IScoreGroup[] IScoreProvider.GetScoreGroups()
        {
            return new IScoreGroup[0];
        }

        /// <summary>
        /// Undocumented ImagerLink API method SetScore
        /// </summary>
        /// <param name="plateID"></param>
        /// <param name="wellNumber"></param>
        /// <param name="dropNumber"></param>
        /// <param name="imageBatchID"></param>
        /// <param name="scoreGroupID"></param>
        /// <param name="score"></param>
        void IScoreProvider.SetScore(string plateID, int wellNumber, int dropNumber, int imageBatchID, int scoreGroupID, double score)
        {
            // Intentionally empty
        }


        # endregion

    }
}
