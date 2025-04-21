using MAUSIC.Data.Entities;
using MAUSIC.Managers;

namespace MAUSIC.Services;

public class RecommendationService
{
    private readonly DatabaseManager _databaseManager;

    public RecommendationService(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public async Task<List<RecommendationPairEntity>> GetAllRecommendationPairs()
    {
        var recommendationPairs = await _databaseManager.GetAllItems<RecommendationPairEntity>();

        return recommendationPairs ?? new List<RecommendationPairEntity>();
    }

    public async Task<int> UpdateRecommendationPairs(List<RecommendationPairEntity> recommendationPairs)
    {
        var result = await _databaseManager.SaveItemsAsync(recommendationPairs);

        return result;
    }

    public async Task<int> AddRecommendationPairs(List<RecommendationPairEntity> recommendationPairs)
    {
        // NOTE: save updates existing and ads new items to db
        var result = await _databaseManager.SaveItemsAsync(recommendationPairs);

        return result;
    }
}