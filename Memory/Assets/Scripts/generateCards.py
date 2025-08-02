from PIL import Image, ImageDraw, ImageFont
import os

# Your list of strings
text_list = [
"Exterior",
"Interior",
"Alto",
"Churros",
"Empanada",
"Sopes",
"Tacos",
"Tamales",
"Agua",
"Caffee",
"Cerveza",
"Chocolate",
"Leche",
"Chilaquiles",
"División",
"Effectivo",
"Pago",
"Tarjeta"]

# Output folder
output_dir = "output_images"
os.makedirs(output_dir, exist_ok=True)

# Image size and background
img_size = (1024, 1024)
bg_color = (255, 255, 255)  # white
text_color = (0, 0, 0)      # black

# Load a font (adjust path or use default)
try:
    # font = ImageFont.truetype("Rye-Regular.ttf", 600, encoding='nuic')

    font = ImageFont.truetype(r'Rye-Regular.ttf', 180)
except:
    font = ImageFont.load_default()

for i, text in enumerate(text_list):
    img = Image.new("RGB", img_size, bg_color)
    draw = ImageDraw.Draw(img)

    # Calculate text size and position
    text_size = draw.textbbox((0, 0), text, font=font)
    text_width = text_size[2] - text_size[0]
    text_height = text_size[3] - text_size[1]
    position = ((img_size[0] - text_width) // 2, (img_size[1] - text_height) // 2)

    draw.text(position, text, fill=text_color, font=font)

    filename = os.path.join(output_dir, text + "_text.png")
    img.save(filename)
