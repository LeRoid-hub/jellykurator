package server

import (
	"database/sql"

	"github.com/LeRoid-hub/jellykurator/server/database"
	"github.com/gin-gonic/gin"
)

var (
	Database *sql.DB
	Env      map[string]string
)

func Run(env map[string]string) {
	Env = env

	db, err := database.New()
	if err != nil {
		panic("Failed to connect to the database: " + err.Error())
	}
	Database = db

	r := gin.Default()

	r.GET("/", welcome)

	v1 := r.Group("/v1")
	{
		v1.GET("/", welcome)

		//Plugin interaction endpoints
		v1.POST("/upload", authCheck, upload)
		v1.POST("/checkauth", authCheck, checkAuth)

		//User
		v1.GET("/User/", checkAuth, getUserProfile)
		v1.POST("/NewUser", createUser)
		v1.POST("/AuthenticateUser", authenticateUser)
		v1.GET("/GenerateToken", checkAuth, generateToken)
		v1.PUT("/UpdateUser/:UserID", checkAuth)
		v1.DELETE("/DeleteUser/:UserID", checkAuth)
	}

	if port, ok := env["PORT"]; ok {
		r.Run(":" + port)
	} else {
		r.Run(":8080")
	}

}

func welcome(c *gin.Context) {
	c.JSON(200, gin.H{
		"message": "Welcome to JellyKurator!",
	})
}
