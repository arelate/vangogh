package attr

import "golang.org/x/net/html"

func Val(node *html.Node, attribute string) string {
	if node == nil {
		return ""
	}
	for _, attr := range node.Attr {
		if attr.Key == attribute {
			return attr.Val
		}
	}
	return ""
}
