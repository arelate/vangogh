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
	dstUrl, _ := destinationUrl(pt, media)

	kv, err := kvas.NewClient(dstUrl, ".json")
	if err != nil {
		return err
	}

	for _, id := range kv.All() {
		reader, _ := kv.Get(id)
		var tt title
		_ = json.NewDecoder(reader).Decode(&tt)
		fmt.Println(id, tt.Title)
	}

	return nil
}
