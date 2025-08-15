import { PerspectiveCamera, OrbitControls } from "@react-three/drei";


export const AqiVisualiser = () => {

    const aspect = 800 / 600;

  return (
    <>
      <ambientLight intensity={0.1} />
      <directionalLight color="white" intensity={0.5} position={[0, 3, 5]} />
      <OrbitControls />
      <PerspectiveCamera makeDefault fov={45} aspect={aspect} near={1} far={1000} position={[0, 0, 50]} />
      <mesh rotation={[0.5, 0, 0]}>
        <boxGeometry args={[50, 10, 25]} />
        <meshStandardMaterial
          color={0xffffff}
          opacity={0.05}
          transparent={true}
        />
        <mesh></mesh>
      </mesh>
    </>
  );
};
