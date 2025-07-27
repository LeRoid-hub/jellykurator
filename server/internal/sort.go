package internal

import (
	"fmt"
)

func Sort(data map[string]interface{}) {
	var movielist []Movie
	var tvshowlist []TvShow

	for _, content := range data {
		contentMap, ok := content.(map[string]interface{})
		if !ok {
			fmt.Printf("content is not a map[string]interface{}\n")
			return
		}
		rawdata, ok := contentMap["type"]
		if !ok {
			fmt.Printf("type does not exists \n")
			return
		}

		typevalue, ok := rawdata.(string)
		if !ok {
			fmt.Printf("dateValue is not a string\n")
			return
		}
		fmt.Printf("date value: %s\n", typevalue)

		switch typevalue {
		case "Series":
			tvshow := seriesExtraction(contentMap)
			tvshowlist = append(tvshowlist, tvshow)

		case "Movie":
			movie := movieExtraction(contentMap)
			movielist = append(movielist, movie)
		}
	}

	//TODO: Match und Upload
}
