package mongocl

import (
	"context"
	"github.com/boggydigital/vangogh/internal/gog/local/schema"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
)

func ChangedSince(mongoClient *mongo.Client, ctx context.Context, colName string, timestamp int64) (added []int, modified []int, err error) {

	added, modified = make([]int, 0), make([]int, 0)

	changesCol := mongoClient.Database("vangogh").Collection(colName + "Changes")

	modCur, err := changesCol.Find(ctx, bson.M{"modified": bson.M{"$gt": timestamp}})
	if err != nil {
		return added, modified, err
	}

	for modCur.Next(ctx) {
		var chg schema.Change
		modCur.Decode(&chg)
		if chg.Added > timestamp {
			added = append(added, chg.ID)
		} else {
			modified = append(modified, chg.ID)
		}
	}

	return added, modified, nil
}
