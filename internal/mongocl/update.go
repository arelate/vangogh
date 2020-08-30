package mongocl

import (
	"context"
	"github.com/boggydigital/vangogh/internal/gog/changes"
	"github.com/boggydigital/vangogh/internal/hash"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
)

func Update(mongoClient *mongo.Client, ctx context.Context, colName string, id int, data interface{}) error {

	col := mongoClient.Database(vangoghDatabase).Collection(colName)
	changesCol := mongoClient.Database(vangoghDatabase).Collection(colName + changesSuffix)

	h, err := hash.Sha256(data)
	if err != nil {
		return err
	}

	var chg changes.Change
	err = changesCol.FindOne(ctx, bson.M{"_id": id}).Decode(&chg)
	if err != nil {
		switch err {
		case mongo.ErrNoDocuments:
			// No document has been found - silently handle the error
			break
		default:
			return err
		}
	}

	switch chg.Hash {
	case "":
		_, err := col.InsertOne(ctx, data)
		if err != nil {
			return err
		}
		changesCol.InsertOne(ctx, changes.New(id, h))
	case h:
		// data unchanged. Do nothing.
	default:
		_, err = col.ReplaceOne(ctx, bson.M{"_id": id}, data)
		if err != nil {
			return err
		}

		_, err = changesCol.ReplaceOne(ctx, bson.M{"_id": id}, chg.Update(h))
		if err != nil {
			return err
		}
	}

	return nil
}
