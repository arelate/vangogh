package storage

import (
	"crypto/sha256"
	"encoding/json"
	"fmt"
	"github.com/boggydigital/vangogh/internal/changes"
	"io/ioutil"
	"os"
	"path"
)

func Save(data interface{}, filename string) error {

	dataBytes, err := json.Marshal(data)
	if err != nil {
		return err
	}

	h := sha256.New()
	h.Write(dataBytes)
	bs := h.Sum(nil)
	hs := fmt.Sprintf("%x", bs)

	if changes.Update(filename, hs) {

		dir := path.Dir(filename)
		if _, err := os.Stat(dir); os.IsNotExist(err) {
			os.MkdirAll(dir, 0755)
		}

		return ioutil.WriteFile(filename, dataBytes, 0644)
	}

	return nil
}
