package changes

func Replace(colName string, id int, chg *Change) error {
	//changesCol := mongoClient.Database(vangoghDatabase).Collection(colName + changesSuffix)
	//_, err := changesCol.ReplaceOne(ctx, bson.M{"_id": id}, chg)
	//return err
	return nil
}
