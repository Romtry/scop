namespace Scop
{
    partial class Program
    {
		// ======= SHADERS 3D =======
        private static readonly string VertexShaderSource3D = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec2 aTextureCoord;
        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;
        out vec2 frag_texCoords;
        void main()
        {
            gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
            frag_texCoords = aTextureCoord;
        }";

        private static readonly string FragmentShaderSource3D = @"
        #version 330 core
        uniform sampler2D uTexture;
        in vec2 frag_texCoords;
        out vec4 out_color;
        void main()
        {
            out_color = texture(uTexture, frag_texCoords);
        }";

        // ======= GÉOMÉTRIE 3D (cube simple) =======
        private static readonly float[] Vertices3D =
        {
            // face avant
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            // face arrière
            -0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        };

        private static readonly uint[] Indices3D =
        {
            0, 1, 2,  2, 3, 0, // avant
            4, 5, 6,  6, 7, 4, // arrière
            0, 4, 7,  7, 3, 0, // gauche
            1, 5, 6,  6, 2, 1, // droite
            3, 2, 6,  6, 7, 3, // haut
            0, 1, 5,  5, 4, 0  // bas
        };

        // pour la rotation 3D
        private static float _angle = 0f;

        private static float[] _currentVertices;
        private static uint[]  _currentIndices;
	}
}
