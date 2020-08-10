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

func Load(filename string) (data []byte, err error) {
	data = nil
	if _, e := os.Stat(filename); e == nil {

		data, err = ioutil.ReadFile(filename)
		if err != nil {
			return data, err
		}
	}
	return data, nil
}
