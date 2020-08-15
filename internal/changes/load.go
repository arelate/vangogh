package changes

import (
	"encoding/json"
	"io/ioutil"
	"os"
)

func Load() error {
	if _, e := os.Stat(Filename); e == nil {
		changeBytes, err := ioutil.ReadFile(Filename)
		if err != nil {
			return err
		}
		return json.Unmarshal(changeBytes, &changes)
	} else {
		changes = make(map[string]Change)
	}
	return nil
}
