package validation

import "encoding/xml"

type Chunk struct {
	XMLName xml.Name `xml:"chunk"`
	ID      int      `xml:"id,attr"`
	From    int      `xml:"from,attr"`
	To      int      `xml:"to,attr"`
	Method  string   `xml:"method,attr"`
	Value   string   `xml:",innerxml"`
}
