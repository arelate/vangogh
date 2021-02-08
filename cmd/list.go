package cmd

import (
	"encoding/json"
	"fmt"
	"github.com/boggydigital/kvas"
)

type title struct {
	Title string `json:"title"`
}

func List(pt, media string) error {
	dstUrl, err := destinationUrl(pt, media)
	if err != nil {
		return err
	}

	kv, err := kvas.NewClient(dstUrl, ".json")
	if err != nil {
		return err
	}

	for _, id := range kv.All() {
		reader, err := kv.Get(id)
		if err != nil {
			return err
		}
		var tt title
		err = json.NewDecoder(reader).Decode(&tt)
		if err != nil {
			return err
		}
		fmt.Println(id, tt.Title)
	}

	return nil
}
