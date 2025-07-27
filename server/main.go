package main

import (
	"github.com/LeRoid-hub/jellykurator/server/server"
)

func main() {
	//env := config.Load()

	//database.SetEnv(env)

	server.Run(map[string]string{})
}
