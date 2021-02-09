package cmd

import (
	"encoding/json"
	"fmt"
	"github.com/boggydigital/kvas"
	"strings"
)

type titled struct {
	Title string `json:"title"`
}

func List(ids []string, title string, pt, media string) error {
	dstUrl, err := destinationUrl(pt, media)
	if err != nil {
		return err
	}

	kv, err := kvas.NewLocal(dstUrl, ".json")
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		ids = kv.All()
	}

	for _, id := range ids {
		rc, err := kv.Get(id)
		if err != nil {
			return err
		}
		var tt titled
		err = json.NewDecoder(rc).Decode(&tt)
		if err != nil {
			return err
		}

		if err := rc.Close(); err != nil {
			return err
		}

		if title != "" && !strings.Contains(
			strings.ToLower(tt.Title),
			strings.ToLower(title)) {
			continue
		}

		fmt.Println(id, tt.Title)
	}

	return nil
}
