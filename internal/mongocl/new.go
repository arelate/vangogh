package mongocl

import (
	"github.com/boggydigital/vangogh/internal/cfg"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func New() (*mongo.Client, error) {

	cfg, err := cfg.Current()
	if err != nil {
		return nil, err
	}

	client, err := mongo.NewClient(
		options.Client().ApplyURI(cfg.Mongo.Conn))
	if err != nil {
		return nil, err
	}

	return client, nil
}
