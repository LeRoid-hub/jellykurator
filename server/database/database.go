package database

import (
	"database/sql"
	"os"
	"strings"
)

type DATABASE struct {
	Host     string
	User     string
	Password string
	Name     string
	Port     string
}

var (
	database DATABASE
)

const (
	SqlFile = "./database/jellykurator.sql"
)

func SetEnv(env map[string]string) {
	var db DATABASE

	db.Host = env["DB_HOST"]
	db.User = env["DB_USER"]
	db.Password = env["DB_PASSWORD"]
	db.Name = env["DB_NAME"]
	db.Port = env["DB_PORT"]

	database = db

	checkDatabase()
}

func New() (*sql.DB, error) {
	conn, err := sql.Open("pgx", connect())
	if err != nil {
		return nil, err
	}
	return conn, nil
}

func connect() string {
	return "postgres://" + database.User + ":" + database.Password + "@" + database.Host + ":" + database.Port + "/" + database.Name + "?sslmode=disable"
	//return "user=" + database.User + " password=" + database.Password + " host=" + database.Host + " dbname=" + database.Name + " sslmode=disable"
}

func checkDatabase() {
	conn, err := sql.Open("pgx", "postgres://"+database.User+":"+database.Password+"@"+database.Host+":"+database.Port+"/postgres?sslmode=disable")
	if err != nil {
		panic(err)
	}
	defer conn.Close()

	err = conn.QueryRow("SELECT datname FROM pg_database WHERE datname = $1", database.Name).Scan(&database.Name)
	if err != nil {
		if err == sql.ErrNoRows {
			createDatabase()
		} else {
			panic(err)
		}
	}
}

func createDatabase() {
	conn, err := sql.Open("pgx", "postgres://"+database.User+":"+database.Password+"@"+database.Host+":"+database.Port+"/postgres?sslmode=disable")
	if err != nil {
		panic(err)
	}
	defer conn.Close()

	_, err = conn.Exec("CREATE DATABASE " + database.Name)
	if err != nil {
		panic(err)
	}

	createTables()
}

func createTables() {
	sqlFile, err := os.ReadFile(SqlFile)
	if err != nil {
		panic(err)
	}

	sqlStatements := strings.Split(string(sqlFile), ";")

	conn, err := sql.Open("pgx", connect())

	if err != nil {
		panic(err)
	}

	defer conn.Close()

	for _, statement := range sqlStatements {
		_, err := conn.Exec(statement)
		if err != nil {
			panic(err)
		}
	}

}

func NewUser(database *sql.DB, user User) error {
	_, err := database.Exec("INSERT INTO users (name, password) VALUES ($1, $2)", user.Name, user.Password)
	if err != nil {
		return err
	}
	return nil

}

func UpdateUser(database *sql.DB, user User) error {
	_, err := database.Exec("UPDATE users SET name = $1, password = $2 WHERE id = $3", user.Name, user.Password, user.ID)
	if err != nil {
		return err
	}
	return nil

}

func DeleteUser(database *sql.DB, id string) error {
	_, err := database.Exec("DELETE FROM users WHERE id = $1", id)
	if err != nil {
		return err
	}
	return nil

}

func GetUser(database *sql.DB, id string) (User, error) {
	var user User
	err := database.QueryRow("SELECT * FROM users WHERE id = $1", id).Scan(&user.ID, &user.Name, &user.Password)
	if err != nil {
		return user, err
	}
	return user, nil
}

func GetUserByName(database *sql.DB, name string) (User, error) {
	var user User
	err := database.QueryRow("SELECT * FROM users WHERE name = $1", name).Scan(&user.ID, &user.Name, &user.Password)
	if err != nil {
		return user, err
	}
	return user, nil

}

func AuthenticateUser(database *sql.DB, name string, password string) (User, error) {
	var user User
	err := database.QueryRow("SELECT * FROM users WHERE name = $1 AND password = $2", name, password).Scan(&user.ID, &user.Name, &user.Password)
	if err != nil {
		return user, err
	}
	return user, nil

}

func AuthenticateUserByToken(database *sql.DB, token string) (User, error) {
	var user User
	err := database.QueryRow("SELECT * FROM users WHERE token = $1", token).Scan(&user.ID, &user.Name, &user.Password)
	if err != nil {
		return user, err
	}
	return user, nil
}
