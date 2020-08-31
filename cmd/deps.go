package cmd

import (
	"context"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

type FetchDeps struct {
	HttpClient  *http.Client
	MongoClient *mongo.Client
	Ctx         context.Context
	Media       media.Type
	Product     string
	Collection  string
}
