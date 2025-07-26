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
}

type Season struct {
	ID           int
	Name         string
	SeasonNumber int
}

type Episodes struct {
	ID            int
	Name          string
	EpisodeNumber int
}
