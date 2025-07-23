package main

import (
	"github.com/LeRoid-hub/jellykurator/server/config"
	"github.com/LeRoid-hub/jellykurator/server/database"
	"github.com/LeRoid-hub/jellykurator/server/server"
)

func main() {
	env := config.Load()

	database.SetEnv(env)

	server.Run(env)
}
