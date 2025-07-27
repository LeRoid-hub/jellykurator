package internal

import "fmt"

func movieExtraction(movie map[string]interface{}) Movie {

	return Movie{}
}

func extractInt(data map[string]interface{}, key string) int {
	rawdata, ok := data[key]
	if !ok {
		fmt.Printf("type does not exists \n")
		return 0
	}

	typevalue, ok := rawdata.(int)
	if !ok {
		fmt.Printf("dateValue is not a string\n")
		return 0
	}
	fmt.Printf("date value: %s\n", typevalue)
	return typevalue
}

func extractString(data map[string]interface{}, key string) string {
	rawdata, ok := data[key]
	if !ok {
		fmt.Printf("type does not exists \n")
		return ""
	}

	typevalue, ok := rawdata.(string)
	if !ok {
		fmt.Printf("dateValue is not a string\n")
		return ""
	}
	fmt.Printf("date value: %s\n", typevalue)
	return typevalue
}
