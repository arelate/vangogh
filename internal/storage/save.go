package storage

import (
	"encoding/json"
	"io/ioutil"
	"os"
	"path"
)

func Save(data interface{}, filename string) error {

	dir := path.Dir(filename)
	if _, err := os.Stat(dir); os.IsNotExist(err) {
		os.MkdirAll(dir, 0755)
	}

	dataBytes, err := json.Marshal(data)
	if err != nil {
		return err
	}
	return ioutil.WriteFile(filename, dataBytes, 0644)
}
