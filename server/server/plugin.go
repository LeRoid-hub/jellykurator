package server

import (
	"fmt"

	"github.com/gin-gonic/gin"
)

func upload(c *gin.Context) {
	var jsondata map[string]interface{}

	if err := c.ShouldBindJSON(&jsondata); err != nil {
		fmt.Printf("Error recieving \n")
	}

	fmt.Println(jsondata)

	//internal.Sort(jsondata)

	// Handle file upload logic here
	c.JSON(200, gin.H{
		"message": "File uploaded successfully!",
	})
}

func checkAuth(c *gin.Context) {
	// Handle authentication check logic here
	c.JSON(200, gin.H{
		"message": "Authentication check successful!",
	})
}
