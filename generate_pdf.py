import markdown
from weasyprint import HTML, CSS

# Read markdown
with open('TechnicalKnowladge.md', 'r') as f:
    text = f.read()

# Convert markdown to HTML
html_content = markdown.markdown(text, extensions=['extra', 'tables'])

# Wrap in a basic HTML template with some styling
full_html = f"""
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            font-size: 14px;
            line-height: 1.6;
            color: #333;
            margin: 20px;
        }}
        h1 {{
            color: #0056b3;
            border-bottom: 2px solid #0056b3;
            padding-bottom: 5px;
        }}
        h2 {{
            color: #d9534f;
            margin-top: 30px;
        }}
        code {{
            background-color: #f4f4f4;
            padding: 2px 5px;
            border-radius: 4px;
            font-family: 'Courier New', Courier, monospace;
        }}
        ul {{
            margin-bottom: 15px;
        }}
        li {{
            margin-bottom: 5px;
        }}
        strong {{
            color: #000;
        }}
    </style>
</head>
<body>
    {html_content}
</body>
</html>
"""

# Generate PDF
HTML(string=full_html).write_pdf('TechnicalKnowladge.pdf')
print("PDF generated successfully.")
