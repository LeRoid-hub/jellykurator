package internal

type Movie struct {
	Name          string
	OriginalTitle string
	Imdb          string
	Databases     string
}

type TvShow struct {
	ID            int
	Name          string
	OriginalTitle string
	Imdb          string
	Season        []Season
}

type Season struct {
	ID           int
	Name         string
	SeasonNumber int
	Episode      []Episode
}

type Episode struct {
	ID            int
	Name          string
	EpisodeNumber int
}
