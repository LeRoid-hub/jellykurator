package internal

func seriesExtraction(series map[string]interface{}) TvShow {
	seasonExtraction(series)
	return TvShow{}
}

func seasonExtraction(season map[string]interface{}) Season {
	episodeExtraction(season)
	return Season{}
}

func episodeExtraction(episode map[string]interface{}) Episode {
	return Episode{}
}
