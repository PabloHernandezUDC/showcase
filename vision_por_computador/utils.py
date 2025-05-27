# Imports necesarios
import os
import numpy as np
import skimage.color as color
import skimage.io as io


# Función para obtener imágenes de un dirctorio
def get_images_from_dir(dir, convert_to_rgb=True):
    # Listamos todos los archivos en el directorio
    files = os.listdir(dir)
    # Leemos cada archivo de imagen y lo almacenamos en una lista
    images = [io.imread(os.path.join(dir, file)) for file in files]
    
    if convert_to_rgb:
        for i in range(len(images)):
            if len(images[i].shape) < 3:
                images[i] = color.gray2rgb(images[i])
    
    # Devolvemos la lista de imágenes en un array
    try:
        return np.array(images)
    except ValueError:
        print("No todas las imágenes tenían el mismo tamaño. El array será de dtype=object.")
        return np.array(images, dtype="object")
        

# Función para normalizar una imagen
def norm_image(image):
    # Devolvemos todos los valores de la imagen divididos por el valor máximo
    return np.nan_to_num(image / image.max())


# Función para aplanar un array
def custom_flatten(array):
    # Devolvemos el array  ya reorganizado en una sola dimensión
    return array.reshape(-1, array.shape[-1])


# Función para formatear una imagen
def format_image(image):
    # Normalizamos la imagen
    image = norm_image(image)
    
    # Devolvemos la imagen normalizada ya aplanada
    return custom_flatten(image)


# Función para desformatear una imagen
def deformat_image(image_array, size):
    return np.reshape(image_array, size)


# Función para formatear una etiqueta de imagen
def format_label(label_image):
    # Devolvemos la imagende etiqueta ya normalizada y aplanada
    return norm_image(label_image).flatten()


# Función para guardar los mosaicos
def save_images(images, output_dir):
    # Creamos el directorio de salida si no existe
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
    
    # Guardamos cada imagen en el directorio de salida
    for i, image in enumerate(images):
        io.imsave(os.path.join(output_dir, f"image{i}.tif"), image)


def neighbor_similarity(image, k=9, tolerance=0.95):
    # image debe tener 3 canales
    # k debe ser impar
    radius = int((k - 1) / 2)
    height, width, _ = image.shape
    result = np.zeros((height, width))
    
    for row in range(height):
        for col in range(width):
            # tenemos que empezar en -1 porque cada pixel siempre se compara con
            # él mismo, y siempre va a ser similar 
            similar_count = -1
            current_pixel = image[row, col]
            
            top_edge = max(0, row - radius)
            bottom_edge = min(height, row + radius) + 1
            
            left_edge = max(0, col - radius) 
            right_edge = min(width, col + radius) + 1
                        
            neighbor_pixels = image[top_edge:bottom_edge, left_edge:right_edge]
                        
            for neighbor in custom_flatten(neighbor_pixels):
                valid = True
                for channel in range(3):
                    # a tiene que ser el pequeño
                    a, b = current_pixel[channel], neighbor[channel]
                    if a > b:
                        a, b = b, a
                    if (a / b) < tolerance:
                        valid = False
                        break
                    
                if valid:
                    similar_count += 1
            
            result[row, col] = similar_count
                    
    return result


def subsample_data(images, labels, ratio):
    # Calculamos el número de muestras a seleccionar basandonos en la proporción
    num_samples = int(len(images) * ratio)
    # Seleccionamos índices aleatorios para el submuestreo
    indices = np.random.choice(len(images), num_samples, replace=False)
    # Submuestreo de las imágenes y etiquetas usando los índices seleccionados
    subsampled_images = images[indices]
    subsampled_labels = labels[indices]
    
    return subsampled_images, subsampled_labels


def get_histograms(image):
    new_color_image = color.rgb2hsv(image)
    greyscale_image = color.rgb2gray(image)    
    slices = np.array([])
    
    grey_histogram, _ = np.histogram(greyscale_image, bins=128)
    slices = np.append(slices, grey_histogram)
    
    for i in range(3):
        counts, _ = np.histogram(new_color_image[:, :, i], bins=128)
        slices = np.append(slices, counts)

    return slices


def get_features(image):
    features = np.array([])
    
    features = np.append(features, np.mean(image) / 255)
    features = np.append(features, np.mean(color.rgb2gray(image)))
    
    return features