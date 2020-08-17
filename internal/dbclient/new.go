package dbclient

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/cli"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"net/url"
)

func New() (*mongo.Client, error) {

	cfg, err := cfg.Current()
	if err != nil {
		return nil, err
	}

	user, pwd := cfg.MongoDB.User, cfg.MongoDB.Pwd
	if pwd == "" {
		user, pwd, err = cli.Credentials(user)
		pwd = url.PathEscape(pwd)
	}

	port := ""
	if cfg.MongoDB.Port != 0 {
		port = fmt.Sprintf(":%d", cfg.MongoDB.Port)
	}

	mongoURL := url.URL{
		Scheme:   cfg.MongoDB.Schema,
		Host:     cfg.MongoDB.Host + port,
		Path:     cfg.MongoDB.Path,
		RawQuery: cfg.MongoDB.Query,
	}

	creds := options.Credential{
		Username: user,
		Password: pwd,
	}

	client, err := mongo.NewClient(
		options.Client().ApplyURI(mongoURL.String()).SetAuth(creds))
	if err != nil {
		return nil, err
	}

	return client, nil
}
